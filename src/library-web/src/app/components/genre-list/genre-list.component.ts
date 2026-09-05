import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GenreService } from '../../services/genre.service';
import { GenreItem } from '../../models/genre.model';

@Component({
  selector: 'app-genre-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="header-card">
        <div>
          <h2 class="section-title">Genres Management</h2>
          <p class="section-subtitle">Manage genres and categories for the catalog.</p>
        </div>

        <form (ngSubmit)="addGenre()" class="add-form">
          <input
            type="text"
            class="form-input"
            placeholder="New genre name..."
            [ngModel]="newGenreName()"
            (ngModelChange)="newGenreName.set($event)"
            name="newGenreName"
            required
            maxlength="100"
          />
          <button type="submit" class="btn btn-primary" [disabled]="isSubmitting()">
            {{ isSubmitting() ? 'Adding...' : '+ Add Genre' }}
          </button>
        </form>
      </div>

      <div class="toolbar-card search-toolbar">
        <div class="search-box">
          <span class="search-icon">🔍</span>
          <input
            type="text"
            class="form-input"
            placeholder="Search genres by name..."
            [ngModel]="searchTerm()"
            (ngModelChange)="onSearchChange($event)"
          />
        </div>
      </div>

      @if (alertMessage()) {
        <div [class]="'alert alert-' + alertType()">
          <span>{{ alertMessage() }}</span>
          <button class="btn-close-alert" (click)="alertMessage.set(null)">✕</button>
        </div>
      }

      <div class="table-card">
        @if (isLoading()) {
          <div class="loading-state">
            <div class="spinner"></div>
            <p>Loading genres...</p>
          </div>
        } @else if (genres().length === 0) {
          <div class="empty-state">No genres found.</div>
        } @else {
          <table class="data-table">
            <thead>
              <tr>
                <th>Genre Name</th>
                <th>Associated Books</th>
                <th class="actions-col">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (genre of genres(); track genre.id) {
                <tr>
                  @if (editingGenreId() === genre.id) {
                    <td>
                      <input
                        type="text"
                        class="form-input edit-input"
                        [ngModel]="editingGenreName()"
                        (ngModelChange)="editingGenreName.set($event)"
                        (keydown.enter)="saveEdit(genre)"
                        (keydown.escape)="cancelEdit()"
                        required
                        maxlength="100"
                      />
                    </td>
                    <td>
                      <span class="badge">{{ genre.bookCount }} books</span>
                    </td>
                    <td class="actions-col">
                      <button
                        class="btn-action edit"
                        (click)="saveEdit(genre)"
                        [disabled]="isSaving()"
                        title="Save Changes"
                      >
                        💾 Save
                      </button>
                      <button
                        class="btn-action"
                        (click)="cancelEdit()"
                        title="Cancel"
                      >
                        ✕ Cancel
                      </button>
                    </td>
                  } @else {
                    <td class="font-medium">{{ genre.name }}</td>
                    <td>
                      <span class="badge">{{ genre.bookCount }} books</span>
                    </td>
                    <td class="actions-col">
                      <button
                        class="btn-action edit"
                        (click)="startEdit(genre)"
                        title="Edit Genre"
                      >
                        ✏️ Edit
                      </button>
                      <button
                        class="btn-action delete"
                        (click)="deleteGenre(genre)"
                        title="Delete Genre"
                      >
                        🗑️ Delete
                      </button>
                    </td>
                  }
                </tr>
              }
            </tbody>
          </table>
        }
      </div>
    </div>
  `,
  styles: [`
    .header-card {
      padding: 1.25rem 1.5rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-wrap: wrap;
      gap: 1rem;
    }
    .section-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: #0f172a;
      margin: 0;
    }
    .section-subtitle {
      font-size: 0.875rem;
      color: #64748b;
      margin: 0.25rem 0 0 0;
    }
    .add-form {
      display: flex;
      gap: 0.75rem;
    }
    .search-toolbar {
      padding: 1rem 1.25rem;
    }
    .search-box {
      position: relative;
      max-width: 360px;
    }
    .search-icon {
      position: absolute;
      left: 0.75rem;
      top: 50%;
      transform: translateY(-50%);
      font-size: 0.875rem;
      pointer-events: none;
    }
    .search-box .form-input {
      width: 100%;
      padding-left: 2.25rem;
    }
    .font-medium {
      font-weight: 500;
      color: #0f172a;
    }
    .edit-input {
      width: 100%;
      max-width: 320px;
    }
  `]
})
export class GenreListComponent implements OnInit {
  private readonly genreService = inject(GenreService);

  readonly genres = signal<GenreItem[]>([]);
  readonly newGenreName = signal('');
  readonly searchTerm = signal('');
  readonly editingGenreId = signal<string | null>(null);
  readonly editingGenreName = signal('');

  readonly isLoading = signal(false);
  readonly isSubmitting = signal(false);
  readonly isSaving = signal(false);

  readonly alertMessage = signal<string | null>(null);
  readonly alertType = signal<'success' | 'error'>('success');

  ngOnInit(): void {
    this.loadGenres();
  }

  loadGenres(): void {
    this.isLoading.set(true);
    this.genreService.getGenres({
      pageSize: 100,
      searchTerm: this.searchTerm().trim() || undefined
    }).subscribe({
      next: (res) => {
        this.genres.set(res.items);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.showAlert(err.error?.detail ?? 'Failed to load genres.', 'error');
      }
    });
  }

  onSearchChange(term: string): void {
    this.searchTerm.set(term);
    this.loadGenres();
  }

  startEdit(genre: GenreItem): void {
    this.editingGenreId.set(genre.id);
    this.editingGenreName.set(genre.name);
  }

  cancelEdit(): void {
    this.editingGenreId.set(null);
    this.editingGenreName.set('');
  }

  saveEdit(genre: GenreItem): void {
    const newName = this.editingGenreName().trim();
    if (!newName) {
      this.showAlert('Genre name cannot be empty.', 'error');
      return;
    }
    if (newName === genre.name) {
      this.cancelEdit();
      return;
    }

    this.isSaving.set(true);
    this.genreService.updateGenre(genre.id, { name: newName }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.cancelEdit();
        this.showAlert(`Genre updated to "${newName}".`, 'success');
        this.loadGenres();
      },
      error: (err: any) => {
        this.isSaving.set(false);
        this.showAlert(err.error?.detail ?? err.error?.title ?? 'Failed to update genre.', 'error');
      }
    });
  }

  addGenre(): void {
    const name = this.newGenreName().trim();
    if (!name) return;

    this.isSubmitting.set(true);
    this.genreService.createGenre({ name }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.newGenreName.set('');
        this.showAlert(`Genre "${name}" added successfully.`, 'success');
        this.loadGenres();
      },
      error: (err: any) => {
        this.isSubmitting.set(false);
        this.showAlert(err.error?.detail ?? err.error?.title ?? 'Failed to add genre.', 'error');
      }
    });
  }

  deleteGenre(genre: GenreItem): void {
    if (!confirm(`Are you sure you want to delete genre "${genre.name}"?`)) {
      return;
    }

    this.genreService.deleteGenre(genre.id).subscribe({
      next: () => {
        this.showAlert(`Genre "${genre.name}" was deleted.`, 'success');
        this.loadGenres();
      },
      error: (err: any) => {
        this.showAlert(err.error?.detail ?? 'Failed to delete genre.', 'error');
      }
    });
  }

  private showAlert(message: string, type: 'success' | 'error'): void {
    this.alertMessage.set(message);
    this.alertType.set(type);
    setTimeout(() => {
      if (this.alertMessage() === message) {
        this.alertMessage.set(null);
      }
    }, 4000);
  }
}

