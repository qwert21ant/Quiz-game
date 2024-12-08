export function notEmptyRule(input: string) {
  return input !== null && input.length > 0 ? true : "Это поле не должно быть пустым";
}