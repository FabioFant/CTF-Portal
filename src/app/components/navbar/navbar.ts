import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { AuthenticationService } from '../../services/authentication-service';

@Component({
  selector: 'app-navbar',
  imports: [MatToolbarModule, MatButtonModule, RouterLink, MatIconModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  authenticationService = inject(AuthenticationService);
  router = inject(Router);
  
  isAdmin() {
    return this.authenticationService.isAdmin();
  }

  becomeAdmin() {
    this.authenticationService.becomeAdmin();
    this.router.navigateByUrl('/');
  }

  revokeAdmin() {
    this.authenticationService.revokeAdmin();
    this.router.navigateByUrl('/');
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigateByUrl('/login');
  }
}
