import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { SearchComponent } from './search.component';

let comp: SearchComponent;
let fixture: ComponentFixture<SearchComponent>;
let de: DebugElement;
let el: HTMLElement;

describe('SearchComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ SearchComponent ], // declare the test component
      imports: [ SharedModule ]
    });

    fixture = TestBed.createComponent(SearchComponent);

    comp = fixture.componentInstance; // SearchComponent test instance

    de = fixture.debugElement.query(By.css('input'));
    el = de.nativeElement;
  });

  it('should display input field to put telephone', () => {
    fixture.detectChanges();
    expect(el.attributes['type'].value).toEqual('tel');
  });

  it('should display a different test title', () => {
    ///comp.title = 'Test Title';
    fixture.detectChanges();
    expect(el.textContent).toContain('Test Title');
  });
});
