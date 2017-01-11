import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'dashed'
})

export class DashedPipe implements PipeTransform {
  transform(val: number, length = 6) {
    const callNumMasked = `${val}————`;
    return callNumMasked.slice(0, length);
  }
}
