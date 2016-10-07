import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SignInComponent }   from './sign-in.component';
import { signInRouting } from './sign-in.routes';

@NgModule({
    imports: [ CommonModule, signInRouting ],
    declarations: [ SignInComponent ]
})
export class SignInModule { }
