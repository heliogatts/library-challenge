import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthorService } from './author.service';
import { AuthorItem, CreateAuthorRequest, UpdateAuthorRequest } from '../models/author.model';
import { PagedResponse } from '../models/paged-response.model';

describe('AuthorService', () => {
  let service: AuthorService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthorService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(AuthorService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getAuthors should query /api/authors with parameters', () => {
    const mockResponse: PagedResponse<AuthorItem> = {
      items: [{ id: 'auth-1', name: 'George Orwell', bookCount: 1 }],
      page: 1,
      pageSize: 50,
      totalCount: 1,
      totalPages: 1,
      hasNextPage: false,
      hasPreviousPage: false
    };

    service.getAuthors({ page: 1, pageSize: 50, searchTerm: 'George' }).subscribe((res) => {
      expect(res.items.length).toBe(1);
      expect(res.items[0].name).toBe('George Orwell');
    });

    const req = httpTesting.expectOne((r) =>
      r.url === '/api/authors' &&
      r.params.get('page') === '1' &&
      r.params.get('pageSize') === '50' &&
      r.params.get('searchTerm') === 'George'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('createAuthor should post to /api/authors', () => {
    const request: CreateAuthorRequest = { name: 'J.K. Rowling' };

    service.createAuthor(request).subscribe((res) => {
      expect(res.id).toBe('new-auth-id');
    });

    const req = httpTesting.expectOne('/api/authors');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush({ id: 'new-auth-id', name: 'J.K. Rowling' });
  });

  it('updateAuthor should put to /api/authors/{id}', () => {
    const request: UpdateAuthorRequest = { name: 'Arthur Conan Doyle' };

    service.updateAuthor('auth-1', request).subscribe();

    const req = httpTesting.expectOne('/api/authors/auth-1');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(request);
    req.flush(null, { status: 204, statusText: 'No Content' });
  });

  it('deleteAuthor should send delete to /api/authors/{id}', () => {
    service.deleteAuthor('auth-1').subscribe();

    const req = httpTesting.expectOne('/api/authors/auth-1');
    expect(req.request.method).toBe('DELETE');
    req.flush(null, { status: 204, statusText: 'No Content' });
  });
});
