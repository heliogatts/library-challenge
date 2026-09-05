import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { AuthorService } from '../../services/author.service';
import { GenreService } from '../../services/genre.service';
import { BookItem } from '../../models/book.model';
import { AuthorItem } from '../../models/author.model';
import { GenreItem } from '../../models/genre.model';
import { BookFormModalComponent } from '../book-form-modal/book-form-modal.component';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, FormsModule, BookFormModalComponent],
  template: `
    <div class="page-container">
      <!-- Toolbar: Search, Filters & Action -->
      <div class="toolbar-card">
        <div class="search-filter-row">
          <div class="search-box">
            <span class="search-icon">🔍</span>
            <input
              type="text"
              class="form-input"
              placeholder="Search books by title..."
              [ngModel]="searchTerm()"
              (ngModelChange)="onSearchChange($event)"
            />
          </div>

          <div class="filter-group">
            <select
              class="form-select"
              [ngModel]="selectedAuthor()"
              (ngModelChange)="onAuthorFilterChange($event)"
            >
              <option value="">All Authors</option>
              @for (author of authors(); track author.id) {
                <option [value]="author.id">{{ author.name }}</option>
              }
            </select>

            <select
              class="form-select"
              [ngModel]="selectedGenre()"
              (ngModelChange)="onGenreFilterChange($event)"
            >
              <option value="">All Genres</option>
              @for (genre of genres(); track genre.id) {
                <option [value]="genre.id">{{ genre.name }}</option>
              }
            </select>
          </div>

          <button class="btn btn-primary" (click)="openCreateModal()">
            <span>+ Add Book</span>
          </button>
        </div>
      </div>

      <!-- Feedback Messages -->
      @if (alertMessage()) {
        <div [class]="'alert alert-' + alertType()">
          <span>{{ alertMessage() }}</span>
          <button class="btn-close-alert" (click)="alertMessage.set(null)">✕</button>
        </div>
      }

      <!-- Books Table Card -->
      <div class="table-card">
        @if (isLoading()) {
          <div class="loading-state">
            <div class="spinner"></div>
            <p>Loading catalog...</p>
          </div>
        } @else if (books().length === 0) {
          <div class="empty-state">
            <p class="empty-title">No books found</p>
            <p class="empty-desc">Try clearing filters or add a new book to get started.</p>
          </div>
        } @else {
          <div class="table-responsive">
            <table class="data-table">
              <thead>
                <tr>
                  <th class="sortable" (click)="setSorting('title')">
                    Title <span class="sort-icon">{{ getSortIcon('title') }}</span>
                  </th>
                  <th>ISBN</th>
                  <th class="sortable" (click)="setSorting('author')">
                    Author <span class="sort-icon">{{ getSortIcon('author') }}</span>
                  </th>
                  <th class="sortable" (click)="setSorting('genre')">
                    Genre <span class="sort-icon">{{ getSortIcon('genre') }}</span>
                  </th>
                  <th class="sortable" (click)="setSorting('year')">
                    Year <span class="sort-icon">{{ getSortIcon('year') }}</span>
                  </th>
                  <th class="actions-col">Actions</th>
                </tr>
              </thead>
              <tbody>
                @for (book of books(); track book.id) {
                  <tr>
                    <td class="font-medium title-cell">{{ book.title }}</td>
                    <td><code class="isbn-badge">{{ book.isbn }}</code></td>
                    <td>{{ book.authorName }}</td>
                    <td><span class="genre-pill">{{ book.genreName }}</span></td>
                    <td>{{ book.publishedYear }}</td>
                    <td class="actions-col">
                      <button class="btn-action edit" (click)="openEditModal(book.id)" title="Edit">
                        ✏️ Edit
                      </button>
                      <button class="btn-action delete" (click)="deleteBook(book)" title="Delete">
                        🗑️ Delete
                      </button>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          <!-- Pagination Footer -->
          <div class="pagination-footer">
            <span class="page-info">
              Showing page <strong>{{ currentPage() }}</strong> of <strong>{{ totalPages() }}</strong>
              ({{ totalCount() }} total books)
            </span>
            <div class="pagination-btns">
              <button
                class="btn btn-secondary btn-sm"
                [disabled]="!hasPreviousPage() || isLoading()"
                (click)="goToPage(currentPage() - 1)"
              >
                Previous
              </button>
              <button
                class="btn btn-secondary btn-sm"
                [disabled]="!hasNextPage() || isLoading()"
                (click)="goToPage(currentPage() + 1)"
              >
                Next
              </button>
            </div>
          </div>
        }
      </div>

      <!-- Add / Edit Modal -->
      <app-book-form-modal
        [isOpen]="isModalOpen()"
        [bookId]="editingBookId()"
        [authors]="authors()"
        [genres]="genres()"
        (saved)="onBookSaved()"
        (closed)="closeModal()"
      />
    </div>
  `,
  styles: [`
    .toolbar-card {
      padding: 1.25rem;
    }
    .search-filter-row {
      display: flex;
      flex-wrap: wrap;
      gap: 1rem;
      align-items: center;
      justify-content: space-between;
    }
    .search-box {
      position: relative;
      flex: 1;
      min-width: 240px;
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
      padding-left: 2.25rem;
    }
    .filter-group {
      display: flex;
      gap: 0.75rem;
      flex-wrap: wrap;
    }
    .title-cell {
      color: #0f172a;
      font-weight: 600;
    }
    .isbn-badge {
      background: #f1f5f9;
      color: #475569;
      padding: 0.2rem 0.5rem;
      border-radius: 4px;
      font-size: 0.8125rem;
    }
    .genre-pill {
      background: #eff6ff;
      color: #2563eb;
      padding: 0.25rem 0.625rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 500;
    }
    .pagination-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 1.25rem;
      background: #f8fafc;
      border-top: 1px solid #e2e8f0;
    }
    .page-info {
      font-size: 0.875rem;
      color: #64748b;
    }
    .pagination-btns {
      display: flex;
      gap: 0.5rem;
    }
    .empty-title {
      font-size: 1.125rem;
      font-weight: 600;
      color: #1e293b;
      margin-bottom: 0.25rem;
    }
    .empty-desc {
      font-size: 0.875rem;
      color: #64748b;
    }
  `]
})
export class BookListComponent implements OnInit {
  private readonly bookService = inject(BookService);
  private readonly authorService = inject(AuthorService);
  private readonly genreService = inject(GenreService);

  readonly books = signal<BookItem[]>([]);
  readonly authors = signal<AuthorItem[]>([]);
  readonly genres = signal<GenreItem[]>([]);

  readonly searchTerm = signal('');
  readonly selectedAuthor = signal('');
  readonly selectedGenre = signal('');

  readonly sortBy = signal<string>('title');
  readonly sortDirection = signal<'asc' | 'desc'>('asc');

  readonly currentPage = signal(1);
  readonly pageSize = signal(10);
  readonly totalCount = signal(0);
  readonly totalPages = signal(1);
  readonly hasNextPage = signal(false);
  readonly hasPreviousPage = signal(false);

  readonly isLoading = signal(false);
  readonly isModalOpen = signal(false);
  readonly editingBookId = signal<string | null>(null);

  readonly alertMessage = signal<string | null>(null);
  readonly alertType = signal<'success' | 'error'>('success');

  ngOnInit(): void {
    this.loadFilters();
    this.loadBooks();
  }

  loadFilters(): void {
    this.authorService.getAuthors({ pageSize: 100 }).subscribe({
      next: (res) => this.authors.set(res.items),
      error: () => {}
    });

    this.genreService.getGenres({ pageSize: 100 }).subscribe({
      next: (res) => this.genres.set(res.items),
      error: () => {}
    });
  }

  loadBooks(): void {
    this.isLoading.set(true);
    this.bookService.getBooks({
      page: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm().trim() || undefined,
      authorId: this.selectedAuthor() || undefined,
      genreId: this.selectedGenre() || undefined,
      sortBy: this.sortBy(),
      sortDirection: this.sortDirection()
    }).subscribe({
      next: (res) => {
        this.books.set(res.items);
        this.currentPage.set(res.page);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.hasNextPage.set(res.hasNextPage);
        this.hasPreviousPage.set(res.hasPreviousPage);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.showAlert(err.error?.detail ?? 'Failed to load books.', 'error');
      }
    });
  }

  onSearchChange(term: string): void {
    this.searchTerm.set(term);
    this.currentPage.set(1);
    this.loadBooks();
  }

  onAuthorFilterChange(authorId: string): void {
    this.selectedAuthor.set(authorId);
    this.currentPage.set(1);
    this.loadBooks();
  }

  onGenreFilterChange(genreId: string): void {
    this.selectedGenre.set(genreId);
    this.currentPage.set(1);
    this.loadBooks();
  }

  setSorting(column: string): void {
    if (this.sortBy() === column) {
      this.sortDirection.update((dir) => (dir === 'asc' ? 'desc' : 'asc'));
    } else {
      this.sortBy.set(column);
      this.sortDirection.set('asc');
    }
    this.currentPage.set(1);
    this.loadBooks();
  }

  getSortIcon(column: string): string {
    if (this.sortBy() !== column) return '↕';
    return this.sortDirection() === 'asc' ? '▲' : '▼';
  }

  goToPage(page: number): void {
    this.currentPage.set(page);
    this.loadBooks();
  }

  openCreateModal(): void {
    this.editingBookId.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(bookId: string): void {
    this.editingBookId.set(bookId);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.editingBookId.set(null);
  }

  onBookSaved(): void {
    this.showAlert(
      this.editingBookId() ? 'Book updated successfully!' : 'Book created successfully!',
      'success'
    );
    this.loadBooks();
  }

  deleteBook(book: BookItem): void {
    if (!confirm(`Are you sure you want to delete "${book.title}"?`)) {
      return;
    }

    this.bookService.deleteBook(book.id).subscribe({
      next: () => {
        this.showAlert(`"${book.title}" was deleted.`, 'success');
        this.loadBooks();
      },
      error: (err: any) => {
        this.showAlert(err.error?.detail ?? 'Failed to delete book.', 'error');
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
