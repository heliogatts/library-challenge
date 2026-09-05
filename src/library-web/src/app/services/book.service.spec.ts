import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { BookService } from './book.service';
import { BookDetail, BookItem, CreateBookRequest, UpdateBookRequest } from '../models/book.model';
import { PagedResponse } from '../models/paged-response.model';

describe('BookService', () => {
  let service: BookService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        BookService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(BookService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getBooks should query /api/books with pagination and filter parameters', () => {
    const mockResponse: PagedResponse<BookItem> = {
      items: [
        {
          id: '1',
          title: '1984',
          isbn: '9780451524935',
          publishedYear: 1949,
          authorName: 'George Orwell',
          genreName: 'Fiction'
        }
      ],
      page: 1,
      pageSize: 10,
      totalCount: 1,
      totalPages: 1,
      hasNextPage: false,
      hasPreviousPage: false
    };

    service.getBooks({
      page: 1,
      pageSize: 10,
      searchTerm: '1984',
      sortBy: 'title',
      sortDirection: 'asc'
    }).subscribe((res) => {
      expect(res.items.length).toBe(1);
      expect(res.items[0].title).toBe('1984');
    });

    const req = httpTesting.expectOne((r) =>
      r.url === '/api/books' &&
      r.params.get('page') === '1' &&
      r.params.get('pageSize') === '10' &&
      r.params.get('searchTerm') === '1984' &&
      r.params.get('sortBy') === 'title' &&
      r.params.get('sortDirection') === 'asc'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('getBookById should query /api/books/{id}', () => {
    const mockBook: BookDetail = {
      id: 'book-123',
      title: 'Foundation',
      isbn: '9780553293357',
      publishedYear: 1951,
      description: 'Sci-fi classic',
      authorId: 'auth-1',
      authorName: 'Isaac Asimov',
      genreId: 'gen-1',
      genreName: 'Science Fiction'
    };

    service.getBookById('book-123').subscribe((book) => {
      expect(book.title).toBe('Foundation');
    });

    const req = httpTesting.expectOne('/api/books/book-123');
    expect(req.request.method).toBe('GET');
    req.flush(mockBook);
  });

  it('createBook should post to /api/books', () => {
    const request: CreateBookRequest = {
      title: 'New Book',
      isbn: '9780451524935',
      publishedYear: 2024,
      authorId: 'auth-1',
      genreId: 'gen-1'
    };

    service.createBook(request).subscribe((res) => {
      expect(res.id).toBe('new-id');
    });

    const req = httpTesting.expectOne('/api/books');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush({ id: 'new-id', title: 'New Book' });
  });

  it('updateBook should put to /api/books/{id}', () => {
    const request: UpdateBookRequest = {
      title: 'Updated Title',
      isbn: '9780451524935',
      publishedYear: 2024,
      authorId: 'auth-1',
      genreId: 'gen-1'
    };

    service.updateBook('book-123', request).subscribe();

    const req = httpTesting.expectOne('/api/books/book-123');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(request);
    req.flush(null, { status: 204, statusText: 'No Content' });
  });

  it('deleteBook should send delete to /api/books/{id}', () => {
    service.deleteBook('book-123').subscribe();

    const req = httpTesting.expectOne('/api/books/book-123');
    expect(req.request.method).toBe('DELETE');
    req.flush(null, { status: 204, statusText: 'No Content' });
  });
});
