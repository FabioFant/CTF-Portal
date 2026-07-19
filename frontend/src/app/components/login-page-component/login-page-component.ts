import { Component, inject } from '@angular/core';
import { AuthenticationService } from '../../services/authentication-service';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-page-component',
  imports: [MatButtonModule],
  templateUrl: './login-page-component.html',
  styleUrl: './login-page-component.css',
})
export class LoginPageComponent {
  authenticationService = inject(AuthenticationService);
  router = inject(Router);

  login() { 
    this.authenticationService.login();
    this.router.navigateByUrl('/');
  }
}
