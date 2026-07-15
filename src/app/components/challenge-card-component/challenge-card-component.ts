import { Component, input, output } from '@angular/core';
import { Challenge } from '../../models/challenge';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from "@angular/router";
import { ChallengeService } from '../../services/challenge-service';

@Component({
  selector: 'app-challenge-card-component',
  imports: [CommonModule, MatCardModule, MatChipsModule, MatButtonModule, RouterLink],
  templateUrl: './challenge-card-component.html',
  styleUrl: './challenge-card-component.css',
})
export class ChallengeCardComponent {
  challenge = input.required<Challenge>();
  onSolved = output<number>();

  constructor(private challengeService : ChallengeService) {

  }

  challengeSolved() {
    this.onSolved.emit(this.challenge().id);
    //this.challengeService.solveChallenge(this.challenge().id); <-- cleaner, thanks to signals we can omess output
  }
}
