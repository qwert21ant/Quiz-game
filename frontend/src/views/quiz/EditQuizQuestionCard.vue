<template>
  <v-container max-width="1000px">
    <v-card>
      <v-card-text>
        <div class="text-h6">Тип вопроса:</div>
        <v-radio-group
          class="ml-5"
          v-model="question.type"
          inline
        >
          <v-radio
            :value="QuizQuestionType.Choise"
            label="Choise"
          />
          <v-radio
            :value="QuizQuestionType.Text"
            label="Text"
          />
        </v-radio-group>
        <v-text-field
          v-model="question.text"
          label="Текст вопроса"
          :rules="[notEmptyRule]"
        />
        <v-text-field
          v-if="question.type === QuizQuestionType.Text"
          v-model="question.answer"
          label="Ответ"
          :rules="[notEmptyRule]"
        />
        <div v-if="question.type === QuizQuestionType.Choise">
          <div class="text-h6">Варианты ответа:</div>
          <v-list>
            <v-list-item
              v-for="(option, ind) in question.options"
              prepend-icon="mdi-circle-medium"
            >
              <v-list-item-title>
                {{ option }}
              </v-list-item-title>
              <template #append>
                <v-btn
                  color="error"
                  variant="outlined"
                  icon="mdi-delete"
                  size="small"
                  @click="removeOption(ind)"
                />
              </template>
            </v-list-item>
            <v-list-item
              prepend-icon="mdi-circle-medium"
            >
              <v-list-item-title>
                <v-text-field
                  v-model="newOption"
                  class="mr-3"
                  variant="outlined"
                  hide-details
                  density="compact"
                />
              </v-list-item-title>
              <template #append>
                <v-btn
                  icon="mdi-plus"
                  size="small"
                  :disabled="newOption.length == 0"
                  @click="addNewOption"
                />
              </template>
            </v-list-item>
          </v-list>
          <v-select
            v-model="answerOption"
            label="Правильный вариант"
            :items="question.options"
          />
        </div>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn 
          text="Сохранить"
          variant="elevated"
          :disabled="!isValid"
          @click="save"
        />
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import QuizQuestion from '@/models/quiz/QuizQuestion';
import QuizQuestionType from '@/models/quiz/QuizQuestionType';
import { defineComponent, PropType } from 'vue';

export default defineComponent({
  name: "EditQuizQuestionCard",
  props: {
    question: Object as PropType<QuizQuestion>,
  },
  emits: ["save"],
  data() {
    return {
      QuizQuestionType,
      newOption: "" as string,
      answerOption: "" as string,
    };
  },
  computed: {
    isValid(): boolean {
      return this.notEmptyRule(this.question.text) === true
        && (
          this.question.type === QuizQuestionType.Text
          && this.notEmptyRule(this.question.answer) === true
          ||
          this.question.type === QuizQuestionType.Choise
          && this.question.options.length >= 2
          && this.question.options.indexOf(this.answerOption) != -1
        );
    },
  },
  methods: {
    removeOption(ind: number) {
      this.question.options.splice(ind, 1);
    },
    addNewOption() {
      this.question.options.push(this.newOption);
      this.newOption = "";
    },
    save() {
      if (this.question.type === QuizQuestionType.Choise) {
        this.question.answerOptionInd = this.question.options.indexOf(this.answerOption);
      }

      // some checks

      this.$emit("save", this.question);
    },
    notEmptyRule(input: string) {
      return input.length > 0 ? true : "Это поле не должно быть пустым";
    }
  },
});
</script>