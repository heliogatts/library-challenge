import { Routes } from '@angular/router';
import { BookListComponent } from './components/book-list/book-list.component';
import { AuthorListComponent } from './components/author-list/author-list.component';
import { GenreListComponent } from './components/genre-list/genre-list.component';

export const routes: Routes = [
  { path: '', redirectTo: 'books', pathMatch: 'full' },
  { path: 'books', component: BookListComponent },
  { path: 'authors', component: AuthorListComponent },
  { path: 'genres', component: GenreListComponent },
  { path: '**', redirectTo: 'books' }
];
