<template>
  <div class="ma-auto" v-if="!isInitialized">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title>
          <div class="mb-1 d-flex align-center">
            <v-btn
              class="mr-3"
              icon="mdi-arrow-left"
              variant="text"
              @click="back"
            />
            <div>Квиз <span class="font-weight-bold">{{ quiz.info.name }}</span></div>
            <v-spacer />
            <v-btn
              color="error"
              variant="outlined"
              prepend-icon="mdi-delete"
              text="Удалить"
              @click="removeQuiz"
            />
          </div>
        </v-card-title>
      </v-card>
    </v-container>
    <template v-for="(question, ind) in quiz.questions">
      <EditQuizQuestionCard
        v-if="ind === editQuestionInd"
        :question="question"
        @save="q => saveQuestion(ind, q)"
        @cancel="cancelQuestion"
      />
      <QuizQuestionCard
        v-else
        :question="question"
        :disabled="editQuestionInd !== null"
        @edit="editQuestion(ind)"
        @remove="removeQuestion(ind)"
      />
    </template>
    <EditQuizQuestionCard
      v-if="newQuestion"
      :question="newQuestion"
      @save="saveNewQuestion"
      @cancel="cancelNewQuestion"
    />
    <v-container
      v-if="!newQuestion"
      class="pt-0 d-flex"
      max-width="1000px"
    >
      <v-spacer />
      <v-btn
        text="Добавить вопрос"
        @click="addNewQuestion"
      />
    </v-container>
  </div>
</template>

<script lang="ts">
import Quiz from '@/models/quiz/Quiz';
import router from '@/router';
import QuizService from '@/services/QuizService';
import { defineComponent } from 'vue';
import EditQuizQuestionCard from './EditQuizQuestionCard.vue';
import QuizQuestion from '@/models/quiz/QuizQuestion';
import QuizQuestionType from '@/models/quiz/QuizQuestionType';
import QuizQuestionCard from './QuizQuestionCard.vue';

export default defineComponent({
  name: "Quiz",
  components: { QuizQuestionCard, EditQuizQuestionCard },
  data() {
    return {
      quizService: new QuizService(),
      quiz: null as Quiz,

      newQuestion: null as QuizQuestion,
      editQuestionInd: null as number,

      isInitialized: false as boolean,
    };
  },
  methods: {
    back() {
      router.push({ path: "/dashboard" });
    },
    async removeQuiz() {
      await this.quizService.removeQuiz(this.$route.params.id);

      router.push({ path: "/dashboard" });
    },
    addNewQuestion() {
      this.newQuestion = {
        type: QuizQuestionType.Choise,
        text: "",
        answer: "",
        options: [],
      };
    },
    async saveNewQuestion() {
      await this.quizService.addQuizQuestion(this.$route.params.id, this.newQuestion);

      this.quiz.questions.push(this.newQuestion);
      this.newQuestion = null;
    },
    editQuestion(ind: number) {
      this.editQuestionInd = ind;
    },
    async removeQuestion(ind: number) {
      await this.quizService.removeQuizQuestion(this.$route.params.id, ind);
      this.quiz.questions.splice(ind, 1);
    },
    async saveQuestion(ind: number, question: QuizQuestion) {
      await this.quizService.changeQuizQuestion(this.$route.params.id, ind, question);

      this.quiz.questions[ind] = question;
      this.editQuestionInd = null;
    },
    cancelQuestion() {
      this.editQuestionInd = null;
    },
    cancelNewQuestion() {
      this.newQuestion = null;
    },
  },
  async mounted() {
    this.quiz = await this.quizService.getQuiz(this.$route.params.id);

    this.isInitialized = true;
  },
});
</script>