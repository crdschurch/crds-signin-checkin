import {
  beforeEachProviders,
  describe,
  expect,
  it,
  inject
} from '@angular/core/testing';
import { FrontEndAppComponent } from '../app/front-end.component';

beforeEachProviders(() => [FrontEndAppComponent]);

describe('App: FrontEnd', () => {
  it('should create the app',
      inject([FrontEndAppComponent], (app: FrontEndAppComponent) => {
    expect(app).toBeTruthy();
  }));

  it('should have as title \'front-end works!\'',
      inject([FrontEndAppComponent], (app: FrontEndAppComponent) => {
    expect(app.title).toEqual('front-end works!');
  }));
});
