import { Component, Input } from '@angular/core';

@Component({
  selector: 'preloader',
  templateUrl: 'preloader.component.html',
  styleUrls: ['preloader.component.scss']
})
export class PreloaderComponent {
  @Input() fullscreen: boolean = false;

  constructor() {
  }

  public isFullscreen(): boolean {
    return this.fullscreen;
  }
}
