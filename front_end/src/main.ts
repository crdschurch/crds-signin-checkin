import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { FrontEndAppComponent, environment } from './app/';

if (environment.production) {
  enableProdMode();
}

bootstrap(FrontEndAppComponent);
