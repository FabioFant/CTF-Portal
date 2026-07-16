import { CanActivateFn, RedirectCommand, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthenticationService } from '../services/authentication-service';

export const adminGuard: CanActivateFn = () => {
  const authenticationService = inject(AuthenticationService);
  const router = inject(Router);

  if(!authenticationService.isAdmin()) {
    const url = router.parseUrl('/');
    return new RedirectCommand(url);
  }
  else {
    return true;
  }
};
