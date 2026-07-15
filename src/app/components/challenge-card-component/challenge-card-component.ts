import { Component, input, output, EventEmitter } from '@angular/core';
import { Challenge } from '../../models/challenge';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-challenge-card-component',
  imports: [CommonModule, MatCardModule, MatChipsModule, MatButtonModule],
  templateUrl: './challenge-card-component.html',
  styleUrl: './challenge-card-component.css',
})
export class ChallengeCardComponent {
  challenge = input.required<Challenge>();
  onSolved = output<number>();

  challengeSolved() {
    this.onSolved.emit(this.challenge().id);
  }
}
