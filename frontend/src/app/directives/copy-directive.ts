import { Directive, inject, input } from '@angular/core';
import { Clipboard } from '@angular/cdk/clipboard';

@Directive({
  selector: '[appCopyToClipboard]',
  standalone: true,
  host: {
    '(click)' : 'onClick()',
  }
})
export class CopyDirective {
  clipboard = inject(Clipboard);
  text = input<string>('', {alias: 'appCopyToClipboard'});

  constructor() { }

  onClick() {
    this.clipboard.copy(this.text());
  }

}
