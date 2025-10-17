import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  template: `
    <div class="home-container">
      <h1>Welcome to Frontend App</h1>
      <p>This is the home page of your Angular application.</p>
      <p>The backend API is connected and ready to use!</p>
    </div>
  `,
  styles: [`
    .home-container {
      padding: 2rem;
      text-align: center;
    }

    h1 {
      color: #333;
      margin-bottom: 1rem;
    }

    p {
      color: #666;
      font-size: 1.1rem;
      margin: 0.5rem 0;
    }
  `]
})
export class HomeComponent {}
