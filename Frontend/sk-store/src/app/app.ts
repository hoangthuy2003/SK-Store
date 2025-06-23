import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layouts/header/header.component'; 
import { FooterComponent } from './layouts/footer/footer.component';
import { NotificationComponent } from './layouts/notification/notification.component';
@Component({
  selector: 'app-root',
  standalone: true,
   imports: [RouterOutlet, HeaderComponent, FooterComponent, NotificationComponent], 
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'sk-store';
}
