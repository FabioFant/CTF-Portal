import { Component, Signal } from '@angular/core';
import { ChallengeCardComponent } from '../challenge-card-component/challenge-card-component';
import { ChallengeService } from '../../services/challenge-service';
import { Challenge } from '../../models/challenge';
import { ChallengeForm } from "../challenge-form/challenge-form";

@Component({
  selector: 'app-dashboard-component',
  imports: [ChallengeCardComponent, ChallengeForm],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css',
})
export class DashboardComponent {
  challenges: Signal<Challenge[]>;

  constructor(private challengeService: ChallengeService) {
    this.challenges = challengeService.getChallenges();
  }

  challengeSolved(id: number) {
    this.challengeService.solveChallenge(id);
  }
}
