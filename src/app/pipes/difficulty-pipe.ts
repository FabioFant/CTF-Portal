import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'difficulty'
})
export class DifficultyPipe implements PipeTransform {

  transform(value: number): string {
    let difficulty : string
    if (value <= 100) {
      difficulty = "🟢 Easy";
    } else if (value > 100 && value < 500) {
      difficulty = "🟡 Medium";
    } else { // > 500
      difficulty = "🔴 Hard";
    }
    return difficulty + ` (${value})`;
  }

}
