import { PhoneNumberPipe } from './phoneNumber.pipe';

describe('PhoneNumberPipe', () => {
  // This pipe is a pure, stateless function so no need for BeforeEach
  let pipe = new PhoneNumberPipe();
  it('transforms "8128128123" to "(812) 812-8123"', () => {
    expect(pipe.transform('8128128123')).toBe('(812) 812-8123');
  });
});
