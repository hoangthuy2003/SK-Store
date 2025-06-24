import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationComponent } from './layouts/notification/notification.component';

@Component({
  selector: 'app-root',
  standalone: true,
  // CHỈ GIỮ LẠI RouterOutlet và NotificationComponent
  imports: [RouterOutlet, NotificationComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'sk-store';
}