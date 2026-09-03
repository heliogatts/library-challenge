export interface BookItem {
  id: string;
  title: string;
  isbn: string;
  publishedYear: number;
  authorName: string;
  genreName: string;
}

export interface BookDetail {
  id: string;
  title: string;
  isbn: string;
  publishedYear: number;
  description?: string;
  authorId: string;
  authorName: string;
  genreId: string;
  genreName: string;
}

export interface CreateBookRequest {
  title: string;
  isbn: string;
  publishedYear: number;
  description?: string;
  authorId: string;
  genreId: string;
}

export interface UpdateBookRequest {
  title: string;
  isbn: string;
  publishedYear: number;
  description?: string;
  authorId: string;
  genreId: string;
}
