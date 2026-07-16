import { Component, inject } from '@angular/core';
import { Challenge } from '../../models/challenge';
import { ChallengeService } from '../../services/challenge-service';
import { ActivatedRoute, Router } from '@angular/router';
import {MatChipsModule} from '@angular/material/chips';
import { CommonModule } from '@angular/common';
import {MatExpansionModule} from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { DifficultyPipe } from '../../pipes/difficulty-pipe';
import { CopyDirective } from '../../directives/copy-directive';
import {MatIconModule} from '@angular/material/icon';

@Component({
  selector: 'app-challenge-details',
  imports: [MatChipsModule, CommonModule, MatExpansionModule, MatButtonModule, DifficultyPipe, CopyDirective, MatIconModule],
  templateUrl: './challenge-details.html',
  styleUrl: './challenge-details.css',
})
export class ChallengeDetails {
  challenge! : Challenge

  challengeService = inject(ChallengeService);
  route = inject(ActivatedRoute);
  router = inject(Router);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    const c = this.challengeService.getChallengeById(id);
    if (c) this.challenge = c;
    else this.router.navigate(['/not-found']);
  }

  solveChallenge() {
    this.challengeService.solveChallenge(this.challenge.id);
  }
}
