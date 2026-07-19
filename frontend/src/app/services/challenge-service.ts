import { Injectable, signal } from '@angular/core';
import { Challenge } from '../models/challenge';
import { MOCK_CHALLENGES } from '../mocks/mock-challenges';

@Injectable({
  providedIn: 'root',
})
export class ChallengeService {
  private challenges = signal<Challenge[]>(MOCK_CHALLENGES);

  private delay(ms: number) { // Fake delay
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  async getChallenges(): Promise<Challenge[]> {
    await this.delay(1500);
    return this.challenges(); 
  }

  async getChallengeById(id: number): Promise<Challenge | undefined> {
    await this.delay(1000);
    return this.challenges().find((challenge) => challenge.id === id);
  }

  async addChallenge(challenge: Challenge, includeDate: boolean): Promise<boolean> {
    await this.delay(1500); 

    if(!challenge || !challenge.title || !challenge.category || !challenge.points) {
      return false;
    }

    if(includeDate) challenge.dateAdded = new Date();
    challenge.solved = false;
    challenge.id = this.challenges().length + 1;

    this.challenges.update(currentChallenges => [...currentChallenges, challenge]);
    return true;
  }

  async solveChallenge(idGiven: number): Promise<void> {
    await this.delay(800);
    
    this.challenges.update(currentChallenges => 
      currentChallenges.map( challenge => {
        if(challenge.id === idGiven) challenge.solved = true;
        return challenge;
      })
    );
  }
}