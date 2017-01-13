import { DashedPipe } from './dashed.pipe';

describe('DashedPipe', () => {
  let pipe = new DashedPipe();

  describe('defaults to length of 6', () => {
    it('transforms "123" to "123———"', () => {
      expect(pipe.transform(123)).toBe('123———');
    });
  });

  describe('allows number of dashes to be passed in', () => {
    it('transforms "456" to "456—"', () => {
      expect(pipe.transform(456, 4)).toBe('456—');
    });
  });

});
