<template>
  <v-container max-width="1000px">
    <v-card>
      <v-card-text>
        <div class="position-relative">
          <div class="text-h6">Тип вопроса: {{ questionType }}</div>
          <div class="text-h6">Вопрос: {{ question.text }}</div>
          <div v-if="question.type == QuizQuestionType.Text" class="text-h6">
            Ответ: {{ question.answer }}
          </div>
          <div v-if="question.type === QuizQuestionType.Choise">
            <div class="text-h6">Варианты ответа:</div>
            <div class="d-flex flex-wrap ml-4">
              <v-card
                v-for="option in question.options"
                class="ma-1"
                density="compact"
              >
                <v-card-title>
                  {{ option }}
                </v-card-title>
              </v-card>
            </div>
            <div class="text-h6">Приавильный вариант: {{ question.options[question.answerOptionInd] }}</div>
          </div>
          <div class="position-absolute top-0 right-0">
            <v-btn
              class="mr-1"
              variant="outlined"
              icon="mdi-pencil"
              size="small"
              :disabled="disabled"
              @click="$emit('edit')"
            />
            <v-btn
              color="error"
              variant="outlined"
              icon="mdi-delete"
              size="small"
              :disabled="disabled"
              @click="$emit('remove')"
            />
          </div>
        </div>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import QuizQuestion from '@/models/quiz/QuizQuestion';
import QuizQuestionType from '@/models/quiz/QuizQuestionType';
import { defineComponent, PropType } from 'vue';

export default defineComponent({
  name: "QuizQuestionCard",
  props: {
    question: Object as PropType<QuizQuestion>,
    disabled: Boolean,
  },
  data() {
    return {
      QuizQuestionType,
    };
  },
  emits: ["edit", "remove"],
  computed: {
    questionType() {
      if (this.question.type === QuizQuestionType.Choise)
        return "Choise";

      return "Text";
    },
  }
});
</script>