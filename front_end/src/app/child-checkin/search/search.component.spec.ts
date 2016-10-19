
import { TestBed, async, inject } from '@angular/core/testing';
import { SearchComponent } from './search.component';

beforeEach(() => {
  TestBed.configureTestingModule({
    declarations: [ TwainComponent ],
    providers:    [ TwainService ],
  });

  fixture = TestBed.createComponent(TwainComponent);
  comp = fixture.componentInstance;

  // TwainService actually injected into the component
  twainService = fixture.debugElement.injector.get(TwainService);

  // Setup spy on the `getQuote` method
  spy = spyOn(twainService, 'getQuote')
        .and.returnValue(Promise.resolve(testQuote));

  // Get the Twain quote element by CSS selector (e.g., by class name)
  de = fixture.debugElement.query(By.css('.twain'));
  el = de.nativeElement;
});