import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BookDetail, BookItem, CreateBookRequest, UpdateBookRequest } from '../models/book.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/books';

  getBooks(params?: {
    page?: number;
    pageSize?: number;
    searchTerm?: string;
    genreId?: string;
    authorId?: string;
    sortBy?: string;
    sortDirection?: string;
  }): Observable<PagedResponse<BookItem>> {
    let httpParams = new HttpParams();
    if (params?.page) httpParams = httpParams.set('page', params.page.toString());
    if (params?.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params?.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params?.genreId) httpParams = httpParams.set('genreId', params.genreId);
    if (params?.authorId) httpParams = httpParams.set('authorId', params.authorId);
    if (params?.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
    if (params?.sortDirection) httpParams = httpParams.set('sortDirection', params.sortDirection);

    return this.http.get<PagedResponse<BookItem>>(this.baseUrl, { params: httpParams });
  }

  getBookById(id: string): Observable<BookDetail> {
    return this.http.get<BookDetail>(`${this.baseUrl}/${id}`);
  }

  createBook(request: CreateBookRequest): Observable<{ id: string; title: string }> {
    return this.http.post<{ id: string; title: string }>(this.baseUrl, request);
  }

  updateBook(id: string, request: UpdateBookRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  deleteBook(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
