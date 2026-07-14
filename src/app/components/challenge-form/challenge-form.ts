import { Component, signal, Signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Challenge } from '../../models/challenge';
import { ChallengeService } from '../../services/challenge-service';
import { ErrorMessage } from '../error-message/error-message';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

const defaultForm : Challenge = {
    id: -1,
    title: '',
    category: '',
    points: 0,
    solved: false,
}

@Component({
  selector: 'app-challenge-form',
  imports: [FormsModule, ErrorMessage, MatInputModule, MatFormFieldModule],
  templateUrl: './challenge-form.html',
  styleUrl: './challenge-form.css',
})
export class ChallengeForm {
  newChallenge : Challenge = { ...defaultForm };
  includeDate : boolean = false;
  errorOccured = signal<boolean>(false);

  constructor(private challengeService: ChallengeService) {

  }

  addChallenge() {
    if (this.challengeService.addChallenge({ ...this.newChallenge }, this.includeDate)) {
      this.errorOccured.set(false);
    }
    else {
      this.errorOccured.set(true);
    }
    this.resetForm();
  }

  resetForm() {
    this.newChallenge = { ...defaultForm };
    this.includeDate = false;
    // this.errorOccured.set(false);
  }
}
