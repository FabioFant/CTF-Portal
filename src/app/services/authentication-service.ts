import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService { // fake auth service
  // should use localStorage
  logged = false;
  admin = false;

  becomeAdmin() {
    if(this.logged) this.admin = true;
  }

  isAdmin(): boolean {
    return this.admin;
  }

  revokeAdmin() {
    this.admin = false;
  }

  login() {
    this.logged = true;
  }

  isLogged(): boolean {
    return this.logged;
  }

  logout() {
    this.logged = false;
    this.admin = false;
  }
}