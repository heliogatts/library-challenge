import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GenreItem, CreateGenreRequest, UpdateGenreRequest } from '../models/genre.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({
  providedIn: 'root'
})
export class GenreService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/genres';

  getGenres(params?: {
    page?: number;
    pageSize?: number;
    searchTerm?: string;
  }): Observable<PagedResponse<GenreItem>> {
    let httpParams = new HttpParams();
    if (params?.page) httpParams = httpParams.set('page', params.page.toString());
    if (params?.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params?.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);

    return this.http.get<PagedResponse<GenreItem>>(this.baseUrl, { params: httpParams });
  }

  createGenre(request: CreateGenreRequest): Observable<{ id: string; name: string }> {
    return this.http.post<{ id: string; name: string }>(this.baseUrl, request);
  }

  updateGenre(id: string, request: UpdateGenreRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  deleteGenre(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
