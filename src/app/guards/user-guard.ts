import { CanActivateFn, Router, RedirectCommand } from '@angular/router';
import { inject } from '@angular/core';
import { AuthenticationService } from '../services/authentication-service';

export const userGuard: CanActivateFn = () => {
  const authenticationService = inject(AuthenticationService);
  const router = inject(Router);

  if(authenticationService.isLogged()) {
    const url = router.parseUrl('/');
    return new RedirectCommand(url);
  }
  else {
    return true;
  }
};
