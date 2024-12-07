export function notEmptyRule(input: string) {
  return input.length > 0 ? true : "Это поле не должно быть пустым";
}