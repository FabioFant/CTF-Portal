import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard-component/dashboard-component';
import { ChallengeForm } from './components/challenge-form/challenge-form';
import { ChallengeDetails } from './components/challenge-details/challenge-details';
import { NotFound } from './components/not-found/not-found';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: "full"
    },
    {
        path: 'dashboard',
        component: DashboardComponent
    },
    {
        path: 'add-challenge',
        component: ChallengeForm
    },
    {
        path: 'challenge/:id',
        component: ChallengeDetails
    },
    {
        path: '**',
        component: NotFound
    }
];
