import { Injectable, inject, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService { // fake auth service
  // should use localStorage
  logged = signal<boolean>(false);
  admin = signal<boolean>(false);

  becomeAdmin() {
    if(this.logged()) this.admin.set(true);
  }

  isAdmin(): boolean {
    return this.admin();
  }

  revokeAdmin() {
    this.admin.set(false);
  }

  login() {
    this.logged.set(true);
  }

  isLogged(): boolean {
    return this.logged();
  }

  logout() {
    this.logged.set(false);
    this.admin.set(false);
  }
}