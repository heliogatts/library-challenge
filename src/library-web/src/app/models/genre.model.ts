export interface GenreItem {
  id: string;
  name: string;
  bookCount: number;
}

export interface CreateGenreRequest {
  name: string;
}

export interface UpdateGenreRequest {
  name: string;
}
