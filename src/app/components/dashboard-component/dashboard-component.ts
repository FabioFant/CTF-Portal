import { Component, inject, Signal } from '@angular/core';
import { ChallengeCardComponent } from '../challenge-card-component/challenge-card-component';
import { ChallengeService } from '../../services/challenge-service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SkeletonCard } from '../skeleton-card/skeleton-card';
import { CTF_CONFIG } from '../../config/ctf.config';

@Component({
  selector: 'app-dashboard-component',
  imports: [ChallengeCardComponent, MatProgressSpinnerModule, SkeletonCard],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css',
})
export class DashboardComponent {
  ctf_config = inject(CTF_CONFIG);
  challengeService = inject(ChallengeService);
  challenges = this.challengeService.getChallenges();

  challengeSolved(id: number) {
    this.challengeService.solveChallenge(id);
  }
}
