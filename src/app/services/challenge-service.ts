import { Injectable, signal, Signal } from '@angular/core';
import { Challenge } from '../models/challenge';
import { MOCK_CHALLENGES } from '../mocks/mock-challenges';

@Injectable({
  providedIn: 'root',
})
export class ChallengeService {
  private challenges = signal<Challenge[]>(MOCK_CHALLENGES);

  getChallenges(): Signal<Challenge[]> {
    return this.challenges.asReadonly();
  }

  addChallenge(challenge: Challenge, includeDate: boolean): boolean {
    if(!challenge || !challenge.title || !challenge.category || !challenge.points) {
      return false;
    }

    if(includeDate) challenge.dateAdded = new Date();
    challenge.solved = false;
    challenge.id = this.challenges().length + 1;

    this.challenges.update(currentChallenges => [...currentChallenges, challenge]);
    return true
  }

  solveChallenge(idGiven: number) {
    this.challenges.update(currentChallenges => 
      currentChallenges.map( challenge => {
        if(challenge.id === idGiven) challenge.solved = true
        return challenge
      })
    )
  }
}
