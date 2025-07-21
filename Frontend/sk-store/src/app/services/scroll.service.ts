import { Injectable } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ScrollService {
  constructor(private router: Router) {
    // Auto scroll to top on route change
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.scrollToTop();
      });
  }

  scrollToTop(behavior: ScrollBehavior = 'smooth'): void {
    window.scrollTo({ 
      top: 0, 
      left: 0, 
      behavior 
    });
  }

  scrollToElement(elementId: string, behavior: ScrollBehavior = 'smooth'): void {
    const element = document.getElementById(elementId);
    if (element) {
      element.scrollIntoView({ 
        behavior, 
        block: 'start' 
      });
    }
  }
}
