import { Component, input, output, inject } from '@angular/core';
import { Challenge } from '../../models/challenge';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from "@angular/router";
import { ChallengeService } from '../../services/challenge-service';
import { DifficultyPipe } from '../../pipes/difficulty-pipe';

@Component({
  selector: 'app-challenge-card-component',
  imports: [CommonModule, MatCardModule, MatChipsModule, MatButtonModule, RouterLink, DifficultyPipe],
  templateUrl: './challenge-card-component.html',
  styleUrl: './challenge-card-component.css',
})
export class ChallengeCardComponent { // Dumb
  challenge = input.required<Challenge>();
  solved = output<number>();

  solveChallenge() {
    this.solved.emit(this.challenge().id);
  }
}
