import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { GenreService } from './genre.service';
import { GenreItem, CreateGenreRequest, UpdateGenreRequest } from '../models/genre.model';
import { PagedResponse } from '../models/paged-response.model';

describe('GenreService', () => {
  let service: GenreService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        GenreService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(GenreService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getGenres should query /api/genres with parameters', () => {
    const mockResponse: PagedResponse<GenreItem> = {
      items: [{ id: 'gen-1', name: 'Fiction', bookCount: 2 }],
      page: 1,
      pageSize: 50,
      totalCount: 1,
      totalPages: 1,
      hasNextPage: false,
      hasPreviousPage: false
    };

    service.getGenres({ page: 1, pageSize: 50, searchTerm: 'Fiction' }).subscribe((res) => {
      expect(res.items.length).toBe(1);
      expect(res.items[0].name).toBe('Fiction');
    });

    const req = httpTesting.expectOne((r) =>
      r.url === '/api/genres' &&
      r.params.get('page') === '1' &&
      r.params.get('pageSize') === '50' &&
      r.params.get('searchTerm') === 'Fiction'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('createGenre should post to /api/genres', () => {
    const request: CreateGenreRequest = { name: 'Thriller' };

    service.createGenre(request).subscribe((res) => {
      expect(res.id).toBe('new-gen-id');
    });

    const req = httpTesting.expectOne('/api/genres');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush({ id: 'new-gen-id', name: 'Thriller' });
  });

  it('updateGenre should put to /api/genres/{id}', () => {
    const request: UpdateGenreRequest = { name: 'Psychological Thriller' };

    service.updateGenre('gen-1', request).subscribe();

    const req = httpTesting.expectOne('/api/genres/gen-1');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(request);
    req.flush(null, { status: 204, statusText: 'No Content' });
  });

  it('deleteGenre should send delete to /api/genres/{id}', () => {
    service.deleteGenre('gen-1').subscribe();

    const req = httpTesting.expectOne('/api/genres/gen-1');
    expect(req.request.method).toBe('DELETE');
    req.flush(null, { status: 204, statusText: 'No Content' });
  });
});
