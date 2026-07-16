import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard-component/dashboard-component';
import { ChallengeForm } from './components/challenge-form/challenge-form';
import { ChallengeDetails } from './components/challenge-details/challenge-details';
import { NotFound } from './components/not-found/not-found';
import { LoginPageComponent } from './components/login-page-component/login-page-component';
import { authGuard } from './guards/auth-guard';
import { adminGuard } from './guards/admin-guard';
import { userGuard } from './guards/user-guard';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: "full"
    },
    {
        path: 'login',
        component: LoginPageComponent,
        canActivate: [userGuard]
    },
    {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [authGuard]
    },
    {
        path: 'add-challenge',
        component: ChallengeForm,
        canActivate: [authGuard, adminGuard]
    },
    {
        path: 'challenge/:id',
        component: ChallengeDetails,
        canActivate: [authGuard]
    },
    {
        path: '**',
        component: NotFound
    }
];
