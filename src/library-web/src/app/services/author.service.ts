import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthorItem, CreateAuthorRequest } from '../models/author.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({
  providedIn: 'root'
})
export class AuthorService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/authors';

  getAuthors(params?: {
    page?: number;
    pageSize?: number;
    searchTerm?: string;
  }): Observable<PagedResponse<AuthorItem>> {
    let httpParams = new HttpParams();
    if (params?.page) httpParams = httpParams.set('page', params.page.toString());
    if (params?.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params?.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);

    return this.http.get<PagedResponse<AuthorItem>>(this.baseUrl, { params: httpParams });
  }

  createAuthor(request: CreateAuthorRequest): Observable<{ id: string; name: string }> {
    return this.http.post<{ id: string; name: string }>(this.baseUrl, request);
  }

  deleteAuthor(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
