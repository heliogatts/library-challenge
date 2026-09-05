import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthorService } from '../../services/author.service';
import { AuthorItem } from '../../models/author.model';

@Component({
  selector: 'app-author-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="header-card">
        <div>
          <h2 class="section-title">Authors Management</h2>
          <p class="section-subtitle">Manage authors in the library catalog.</p>
        </div>

        <form (ngSubmit)="addAuthor()" class="add-form">
          <input
            type="text"
            class="form-input"
            placeholder="New author name..."
            [ngModel]="newAuthorName()"
            (ngModelChange)="newAuthorName.set($event)"
            name="newAuthorName"
            required
            maxlength="200"
          />
          <button type="submit" class="btn btn-primary" [disabled]="isSubmitting()">
            {{ isSubmitting() ? 'Adding...' : '+ Add Author' }}
          </button>
        </form>
      </div>

      <div class="toolbar-card search-toolbar">
        <div class="search-box">
          <span class="search-icon">🔍</span>
          <input
            type="text"
            class="form-input"
            placeholder="Search authors by name..."
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
            <p>Loading authors...</p>
          </div>
        } @else if (authors().length === 0) {
          <div class="empty-state">No authors found.</div>
        } @else {
          <table class="data-table">
            <thead>
              <tr>
                <th>Author Name</th>
                <th>Associated Books</th>
                <th class="actions-col">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (author of authors(); track author.id) {
                <tr>
                  @if (editingAuthorId() === author.id) {
                    <td>
                      <input
                        type="text"
                        class="form-input edit-input"
                        [ngModel]="editingAuthorName()"
                        (ngModelChange)="editingAuthorName.set($event)"
                        (keydown.enter)="saveEdit(author)"
                        (keydown.escape)="cancelEdit()"
                        required
                        maxlength="200"
                      />
                    </td>
                    <td>
                      <span class="badge">{{ author.bookCount }} books</span>
                    </td>
                    <td class="actions-col">
                      <button
                        class="btn-action edit"
                        (click)="saveEdit(author)"
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
                    <td class="font-medium">{{ author.name }}</td>
                    <td>
                      <span class="badge">{{ author.bookCount }} books</span>
                    </td>
                    <td class="actions-col">
                      <button
                        class="btn-action edit"
                        (click)="startEdit(author)"
                        title="Edit Author"
                      >
                        ✏️ Edit
                      </button>
                      <button
                        class="btn-action delete"
                        (click)="deleteAuthor(author)"
                        title="Delete Author"
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
export class AuthorListComponent implements OnInit {
  private readonly authorService = inject(AuthorService);

  readonly authors = signal<AuthorItem[]>([]);
  readonly newAuthorName = signal('');
  readonly searchTerm = signal('');
  readonly editingAuthorId = signal<string | null>(null);
  readonly editingAuthorName = signal('');

  readonly isLoading = signal(false);
  readonly isSubmitting = signal(false);
  readonly isSaving = signal(false);

  readonly alertMessage = signal<string | null>(null);
  readonly alertType = signal<'success' | 'error'>('success');

  ngOnInit(): void {
    this.loadAuthors();
  }

  loadAuthors(): void {
    this.isLoading.set(true);
    this.authorService.getAuthors({
      pageSize: 100,
      searchTerm: this.searchTerm().trim() || undefined
    }).subscribe({
      next: (res) => {
        this.authors.set(res.items);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.showAlert(err.error?.detail ?? 'Failed to load authors.', 'error');
      }
    });
  }

  onSearchChange(term: string): void {
    this.searchTerm.set(term);
    this.loadAuthors();
  }

  startEdit(author: AuthorItem): void {
    this.editingAuthorId.set(author.id);
    this.editingAuthorName.set(author.name);
  }

  cancelEdit(): void {
    this.editingAuthorId.set(null);
    this.editingAuthorName.set('');
  }

  saveEdit(author: AuthorItem): void {
    const newName = this.editingAuthorName().trim();
    if (!newName) {
      this.showAlert('Author name cannot be empty.', 'error');
      return;
    }
    if (newName === author.name) {
      this.cancelEdit();
      return;
    }

    this.isSaving.set(true);
    this.authorService.updateAuthor(author.id, { name: newName }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.cancelEdit();
        this.showAlert(`Author updated to "${newName}".`, 'success');
        this.loadAuthors();
      },
      error: (err: any) => {
        this.isSaving.set(false);
        this.showAlert(err.error?.detail ?? err.error?.title ?? 'Failed to update author.', 'error');
      }
    });
  }

  addAuthor(): void {
    const name = this.newAuthorName().trim();
    if (!name) return;

    this.isSubmitting.set(true);
    this.authorService.createAuthor({ name }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.newAuthorName.set('');
        this.showAlert(`Author "${name}" added successfully.`, 'success');
        this.loadAuthors();
      },
      error: (err: any) => {
        this.isSubmitting.set(false);
        this.showAlert(err.error?.detail ?? err.error?.title ?? 'Failed to add author.', 'error');
      }
    });
  }

  deleteAuthor(author: AuthorItem): void {
    if (!confirm(`Are you sure you want to delete author "${author.name}"?`)) {
      return;
    }

    this.authorService.deleteAuthor(author.id).subscribe({
      next: () => {
        this.showAlert(`Author "${author.name}" was deleted.`, 'success');
        this.loadAuthors();
      },
      error: (err: any) => {
        this.showAlert(err.error?.detail ?? 'Failed to delete author.', 'error');
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

