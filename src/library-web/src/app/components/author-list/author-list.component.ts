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

      @if (alertMessage()) {
        <div [class]="'alert alert-' + alertType()">
          <span>{{ alertMessage() }}</span>
          <button class="btn-close-alert" (click)="alertMessage.set(null)">✕</button>
        </div>
      }

      <div class="table-card">
        @if (isLoading()) {
          <div class="loading-state">Loading authors...</div>
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
                  <td class="font-medium">{{ author.name }}</td>
                  <td>
                    <span class="badge">{{ author.bookCount }} books</span>
                  </td>
                  <td class="actions-col">
                    <button
                      class="btn-action delete"
                      (click)="deleteAuthor(author)"
                      title="Delete Author"
                    >
                      🗑️ Delete
                    </button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-container {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
    }
    .header-card {
      background: #ffffff;
      padding: 1.25rem 1.5rem;
      border-radius: 12px;
      border: 1px solid #e2e8f0;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
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
    .form-input {
      padding: 0.625rem 0.875rem;
      border: 1px solid #cbd5e1;
      border-radius: 8px;
      font-size: 0.875rem;
      outline: none;
      min-width: 240px;
    }
    .btn {
      padding: 0.625rem 1.25rem;
      border-radius: 8px;
      font-size: 0.875rem;
      font-weight: 500;
      cursor: pointer;
      border: none;
      transition: all 0.2s;
      white-space: nowrap;
    }
    .btn-primary {
      background: #2563eb;
      color: #ffffff;
    }
    .btn-primary:hover:not(:disabled) {
      background: #1d4ed8;
    }
    .table-card {
      background: #ffffff;
      border-radius: 12px;
      border: 1px solid #e2e8f0;
      overflow: hidden;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    }
    .data-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
      font-size: 0.875rem;
    }
    .data-table th {
      background: #f8fafc;
      padding: 0.875rem 1.25rem;
      color: #64748b;
      font-weight: 600;
      border-bottom: 1px solid #e2e8f0;
      text-transform: uppercase;
      font-size: 0.75rem;
      letter-spacing: 0.05em;
    }
    .data-table td {
      padding: 1rem 1.25rem;
      border-bottom: 1px solid #f1f5f9;
      color: #334155;
    }
    .badge {
      background: #f1f5f9;
      color: #475569;
      padding: 0.2rem 0.6rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 500;
    }
    .actions-col {
      text-align: right;
    }
    .btn-action.delete {
      background: transparent;
      border: none;
      color: #dc2626;
      cursor: pointer;
      padding: 0.35rem 0.65rem;
      border-radius: 6px;
    }
    .btn-action.delete:hover {
      background: #fef2f2;
    }
    .alert {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.875rem 1.25rem;
      border-radius: 8px;
      font-size: 0.875rem;
    }
    .alert-success {
      background: #f0fdf4;
      border: 1px solid #bbf7d0;
      color: #166534;
    }
    .alert-error {
      background: #fef2f2;
      border: 1px solid #fecaca;
      color: #b91c1c;
    }
    .btn-close-alert {
      background: none;
      border: none;
      cursor: pointer;
      font-size: 1rem;
      color: inherit;
    }
    .loading-state, .empty-state {
      padding: 2.5rem;
      text-align: center;
      color: #64748b;
    }
  `]
})
export class AuthorListComponent implements OnInit {
  private readonly authorService = inject(AuthorService);

  readonly authors = signal<AuthorItem[]>([]);
  readonly newAuthorName = signal('');
  readonly isLoading = signal(false);
  readonly isSubmitting = signal(false);

  readonly alertMessage = signal<string | null>(null);
  readonly alertType = signal<'success' | 'error'>('success');

  ngOnInit(): void {
    this.loadAuthors();
  }

  loadAuthors(): void {
    this.isLoading.set(true);
    this.authorService.getAuthors({ pageSize: 100 }).subscribe({
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
