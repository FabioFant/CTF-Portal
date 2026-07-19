import { Component, inject, resource, computed, signal } from '@angular/core';
import { ChallengeCardComponent } from '../challenge-card-component/challenge-card-component';
import { ChallengeService } from '../../services/challenge-service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SkeletonCard } from '../skeleton-card/skeleton-card';
import { CTF_CONFIG } from '../../config/ctf.config';
import { SearchBarComponent } from '../search-bar-component/search-bar-component';

@Component({
  selector: 'app-dashboard-component',
  imports: [ChallengeCardComponent, MatProgressSpinnerModule, SkeletonCard, SearchBarComponent],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css',
})
export class DashboardComponent { // Smart
  ctf_config = inject(CTF_CONFIG);
  challengeService = inject(ChallengeService);

  searchChallenge = signal<string>('');

  challengeResource = resource({
    loader: () => { return this.challengeService.getChallenges(); }
  })

  filteredChallenges = computed(() => {
    const challenges = this.challengeResource.value() ?? []
    const searchQuery = this.searchChallenge().toLowerCase();

    if(!searchQuery) {
      return challenges
    }

    return challenges.filter(challenge => challenge.title.toLowerCase().includes(searchQuery))
  })

  totalPoints = computed(() => {
    const challenges = this.challengeResource.value() ?? [];
    return challenges
    .filter(challenge => challenge.solved)
    .reduce((sum, challenge) => sum += challenge.points, 0);
  })

  totalChallengeSolved = computed(() => {
    const challenges = this.challengeResource.value() ?? [];
    return `${challenges.filter(challenge => challenge.solved).length} / ${challenges.length}`
  })

  challengeSolved(id: number) {
    this.challengeService.solveChallenge(id);
    this.challengeResource.reload();
  }
}
