import { ComponentFixture, TestBed, async } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SearchComponent } from './search.component';
import { NumberPadComponent } from './number-pad';

let component: SearchComponent;
let fixture: ComponentFixture<SearchComponent>;
let de:      DebugElement;
let el:      HTMLElement;

describe('SearchComponent', () => {
  beforeEach(() => {
    return TestBed.configureTestingModule({
      declarations: [ SearchComponent ],
    })
    .compileComponents().then(() => {
      fixture = TestBed.createComponent(SearchComponent);
      fixture.detectChanges();
    });
  });

  afterEach(() => {
    fixture.destroy();
  });

  it('should contain an input field of type tel', () => {
    de = fixture.debugElement.query(By.css('num-pad-value'));
    el = de.nativeElement;
    expect(el.getAttribute('type')).toContain('type');
  });

});
