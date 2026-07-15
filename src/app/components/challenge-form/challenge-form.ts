import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Challenge } from '../../models/challenge';
import { ChallengeService } from '../../services/challenge-service';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

const defaultForm : Challenge = {
    id: -1,
    title: '',
    category: '',
    points: 0,
    solved: false,
}

@Component({
  selector: 'app-challenge-form',
  imports: [FormsModule, MatInputModule, MatFormFieldModule, MatCheckboxModule, MatButtonModule, MatSnackBarModule],
  templateUrl: './challenge-form.html',
  styleUrl: './challenge-form.css',
})
export class ChallengeForm {
  newChallenge : Challenge = { ...defaultForm };
  includeDate : boolean = false;
  snackBar = inject(MatSnackBar);

  constructor(private challengeService: ChallengeService) {

  }

  addChallenge() {
    if (this.challengeService.addChallenge({ ...this.newChallenge }, this.includeDate)) {
      this.snackBar.open(`Challenge "${this.newChallenge.title}" added.`, "Close", {
        duration: 3000,
      });
    }
    else {
      this.snackBar.open("Invalid challenge.", "Close", {
        duration: 3000,
      });
    }
    this.resetForm();
  }

  resetForm() {
    this.newChallenge = { ...defaultForm };
    this.includeDate = false;
  }
}
