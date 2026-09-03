import { Component, inject, input, output, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { AuthorItem } from '../../models/author.model';
import { GenreItem } from '../../models/genre.model';

@Component({
  selector: 'app-book-form-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    @if (isOpen()) {
      <div class="modal-backdrop" (click)="onBackdropClick($event)">
        <div class="modal-card">
          <div class="modal-header">
            <h3 class="modal-title">{{ bookId() ? 'Edit Book' : 'Add New Book' }}</h3>
            <button type="button" class="btn-close" (click)="closeModal()">✕</button>
          </div>

          @if (errorMessage()) {
            <div class="alert alert-error">
              {{ errorMessage() }}
            </div>
          }

          <form (ngSubmit)="onSubmit()" class="modal-body">
            <div class="form-group">
              <label for="book-title">Title *</label>
              <input
                id="book-title"
                type="text"
                class="form-control"
                [ngModel]="title()"
                (ngModelChange)="title.set($event)"
                name="title"
                required
                maxlength="200"
                placeholder="Enter book title"
              />
            </div>

            <div class="form-row">
              <div class="form-group col">
                <label for="book-isbn">ISBN *</label>
                <input
                  id="book-isbn"
                  type="text"
                  class="form-control"
                  [ngModel]="isbn()"
                  (ngModelChange)="isbn.set($event)"
                  name="isbn"
                  required
                  maxlength="13"
                  placeholder="e.g. 9780451524935"
                />
              </div>

              <div class="form-group col">
                <label for="book-year">Published Year *</label>
                <input
                  id="book-year"
                  type="number"
                  class="form-control"
                  [ngModel]="publishedYear()"
                  (ngModelChange)="publishedYear.set($event)"
                  name="publishedYear"
                  required
                  min="1450"
                  [max]="currentYear"
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group col">
                <label for="book-author">Author *</label>
                <select
                  id="book-author"
                  class="form-control"
                  [ngModel]="authorId()"
                  (ngModelChange)="authorId.set($event)"
                  name="authorId"
                  required
                >
                  <option value="" disabled>Select Author</option>
                  @for (author of authors(); track author.id) {
                    <option [value]="author.id">{{ author.name }}</option>
                  }
                </select>
              </div>

              <div class="form-group col">
                <label for="book-genre">Genre *</label>
                <select
                  id="book-genre"
                  class="form-control"
                  [ngModel]="genreId()"
                  (ngModelChange)="genreId.set($event)"
                  name="genreId"
                  required
                >
                  <option value="" disabled>Select Genre</option>
                  @for (genre of genres(); track genre.id) {
                    <option [value]="genre.id">{{ genre.name }}</option>
                  }
                </select>
              </div>
            </div>

            <div class="form-group">
              <label for="book-description">Description</label>
              <textarea
                id="book-description"
                class="form-control"
                rows="3"
                [ngModel]="description()"
                (ngModelChange)="description.set($event)"
                name="description"
                maxlength="2000"
                placeholder="Optional description or summary..."
              ></textarea>
            </div>

            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="closeModal()" [disabled]="isSubmitting()">
                Cancel
              </button>
              <button type="submit" class="btn btn-primary" [disabled]="isSubmitting()">
                {{ isSubmitting() ? 'Saving...' : (bookId() ? 'Update Book' : 'Create Book') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
  styles: [`
    .modal-backdrop {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      background: rgba(15, 23, 42, 0.6);
      backdrop-filter: blur(4px);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 999;
      padding: 1rem;
    }
    .modal-card {
      background: #ffffff;
      border-radius: 12px;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1);
      width: 100%;
      max-width: 580px;
      max-height: 90vh;
      overflow-y: auto;
      border: 1px solid #e2e8f0;
    }
    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.25rem 1.5rem;
      border-bottom: 1px solid #f1f5f9;
    }
    .modal-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: #0f172a;
      margin: 0;
    }
    .btn-close {
      background: none;
      border: none;
      font-size: 1.25rem;
      color: #64748b;
      cursor: pointer;
      padding: 0.25rem 0.5rem;
      border-radius: 6px;
      line-height: 1;
    }
    .btn-close:hover {
      background: #f1f5f9;
      color: #0f172a;
    }
    .modal-body {
      padding: 1.5rem;
      display: flex;
      flex-direction: column;
      gap: 1.15rem;
    }
    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.35rem;
    }
    .form-row {
      display: flex;
      gap: 1rem;
    }
    .col {
      flex: 1;
    }
    label {
      font-size: 0.875rem;
      font-weight: 500;
      color: #334155;
    }
    .form-control {
      width: 100%;
      padding: 0.625rem 0.875rem;
      border: 1px solid #cbd5e1;
      border-radius: 8px;
      font-size: 0.925rem;
      color: #0f172a;
      background: #ffffff;
      outline: none;
      box-sizing: border-box;
      transition: border-color 0.2s, box-shadow 0.2s;
    }
    .form-control:focus {
      border-color: #3b82f6;
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15);
    }
    .modal-footer {
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
      margin-top: 0.75rem;
    }
    .btn {
      padding: 0.625rem 1.25rem;
      border-radius: 8px;
      font-size: 0.875rem;
      font-weight: 500;
      cursor: pointer;
      border: none;
      transition: all 0.2s;
    }
    .btn-primary {
      background: #2563eb;
      color: #ffffff;
    }
    .btn-primary:hover:not(:disabled) {
      background: #1d4ed8;
    }
    .btn-secondary {
      background: #f1f5f9;
      color: #475569;
    }
    .btn-secondary:hover:not(:disabled) {
      background: #e2e8f0;
    }
    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .alert-error {
      margin: 1rem 1.5rem 0 1.5rem;
      padding: 0.75rem 1rem;
      border-radius: 8px;
      background: #fef2f2;
      border: 1px solid #fecaca;
      color: #b91c1c;
      font-size: 0.875rem;
    }
  `]
})
export class BookFormModalComponent {
  private readonly bookService = inject(BookService);

  readonly isOpen = input(false);
  readonly bookId = input<string | null>(null);
  readonly authors = input<AuthorItem[]>([]);
  readonly genres = input<GenreItem[]>([]);

  readonly saved = output<void>();
  readonly closed = output<void>();

  readonly title = signal('');
  readonly isbn = signal('');
  readonly publishedYear = signal(new Date().getFullYear());
  readonly description = signal('');
  readonly authorId = signal('');
  readonly genreId = signal('');

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly currentYear = new Date().getFullYear();

  constructor() {
    effect(() => {
      const id = this.bookId();
      if (this.isOpen() && id) {
        this.loadBook(id);
      } else if (this.isOpen() && !id) {
        this.resetForm();
      }
    });
  }

  private loadBook(id: string): void {
    this.errorMessage.set(null);
    this.bookService.getBookById(id).subscribe({
      next: (book) => {
        this.title.set(book.title);
        this.isbn.set(book.isbn);
        this.publishedYear.set(book.publishedYear);
        this.description.set(book.description ?? '');
        this.authorId.set(book.authorId);
        this.genreId.set(book.genreId);
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.detail ?? 'Failed to load book details.');
      }
    });
  }

  private resetForm(): void {
    this.title.set('');
    this.isbn.set('');
    this.publishedYear.set(new Date().getFullYear());
    this.description.set('');
    this.authorId.set(this.authors().length > 0 ? this.authors()[0].id : '');
    this.genreId.set(this.genres().length > 0 ? this.genres()[0].id : '');
    this.errorMessage.set(null);
  }

  closeModal(): void {
    this.closed.emit();
  }

  onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.closeModal();
    }
  }

  onSubmit(): void {
    if (!this.title().trim() || !this.isbn().trim() || !this.authorId() || !this.genreId()) {
      this.errorMessage.set('Please fill in all required fields.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const payload = {
      title: this.title().trim(),
      isbn: this.isbn().trim(),
      publishedYear: Number(this.publishedYear()),
      description: this.description().trim() || undefined,
      authorId: this.authorId(),
      genreId: this.genreId()
    };

    const id = this.bookId();
    if (id) {
      this.bookService.updateBook(id, payload).subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.saved.emit();
          this.closeModal();
        },
        error: (err: any) => {
          this.isSubmitting.set(false);
          const detail = err.error?.detail ?? err.error?.title ?? 'An unexpected error occurred.';
          this.errorMessage.set(detail);
        }
      });
    } else {
      this.bookService.createBook(payload).subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.saved.emit();
          this.closeModal();
        },
        error: (err: any) => {
          this.isSubmitting.set(false);
          const detail = err.error?.detail ?? err.error?.title ?? 'An unexpected error occurred.';
          this.errorMessage.set(detail);
        }
      });
    }
  }
}
