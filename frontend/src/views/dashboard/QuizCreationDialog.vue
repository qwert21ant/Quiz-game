<template>
  <v-dialog
    v-model="displayed"
    max-width="600"
  >
    <v-card>
      <v-card-title>Создать квиз</v-card-title>
      <v-card-text style="padding-bottom: 0">
        <v-text-field
          v-model="quizName"
          label="Название квиза"
        />
      </v-card-text>
      <v-card-actions>
        <v-btn
          text="Создать"
          @click="$emit('create', quizName)"
        />
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: "QuizCreationDialog",
  props: {
    modelValue: Boolean,
  },
  emits: ["update:modelValue", "create"],
  data() {
    return {
      quizName: "" as string,
    };
  },
  computed: {
    displayed: {
      get() {
        return this.modelValue;
      },
      set(value) {
        this.$emit("update:modelValue", value);
      }
    },
  },
});
</script>