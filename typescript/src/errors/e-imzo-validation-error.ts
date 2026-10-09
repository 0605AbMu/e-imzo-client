/**
 * Exception thrown when validation fails for input parameters.
 */
export class EImzoValidationError extends Error {
  public readonly fieldName: string;

  constructor(fieldName: string, message: string) {
    super(`${fieldName}: ${message}`);
    this.name = 'EImzoValidationError';
    this.fieldName = fieldName;

    Object.setPrototypeOf(this, EImzoValidationError.prototype);
  }
}
