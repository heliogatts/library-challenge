import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  readonly apiDocsUrl = typeof window !== 'undefined' && window.location.port === '4200'
    ? 'http://localhost:5000/scalar/v1'
    : '/scalar/v1';
}

