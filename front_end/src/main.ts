import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { FrontEndAppComponent, environment } from './app/';
import { ROUTER_PROVIDERS } from '@angular/router';

if (environment.production) {
  enableProdMode();
}

bootstrap(FrontEndAppComponent, [ROUTER_PROVIDERS]);
